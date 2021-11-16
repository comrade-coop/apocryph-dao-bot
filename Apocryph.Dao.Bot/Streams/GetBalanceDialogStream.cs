using System;
using System.Collections.Generic;
using System.Linq;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Nethereum.StandardTokenEIP20;
using Nethereum.Util;
using Perper.Model;
using Serilog;

namespace Apocryph.Dao.Bot.Streams
{
    public class GetBalanceDialogStream
    {
        private readonly StandardTokenService _tokenService;
        private readonly IState _state;
        private readonly IOptions<Configuration.Dao> _options;
        private readonly GetBalanceMessageValidator _validator;

        public GetBalanceDialogStream(IState state, StandardTokenService tokenService, IOptions<Configuration.Dao> options)
        {
            _state = state;
            _options = options;
            _tokenService = tokenService;
            _validator = new GetBalanceMessageValidator(state);
        }

        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<IInboundMessage> messages)
        {
            await foreach (var message in messages.Where(m => m is GetBalanceMessage).Cast<GetBalanceMessage>())
            {
                IOutboundMessage outputMessage = new ErrorMessage(default, default, default);
                try
                {
                    var balanceLog = Log.ForContext("Session", message.UserId);

                    var result = await _validator.ValidateAsync(message);
                    if (result.IsValid)
                    {
                        var address = await _state.GetAddress(message.UserId);
                        var senderBalance = await _tokenService.BalanceOfQueryAsync(address);
                        outputMessage = new BalanceMessage(
                            UserId: message.UserId,
                            Amount: UnitConversion.Convert.FromWei(senderBalance));

                        balanceLog.Information("Processed {@Message}", message);
                    }
                    else
                    {
                        var errorMessage = new ErrorMessage(default, message.UserId, result.Errors.Select(x => x.ErrorMessage).ToArray());
                        outputMessage = errorMessage;

                        balanceLog.Information("Processed {@ErrorMessage}", errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to process {@Message}", message);
                }

                yield return outputMessage;
            }
        }
    }
}