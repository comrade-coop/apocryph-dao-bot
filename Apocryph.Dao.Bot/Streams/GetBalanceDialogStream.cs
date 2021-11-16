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
        private readonly GetBalanceMessageValidator _validator;

        public GetBalanceDialogStream(IState state, StandardTokenService tokenService)
        {
            _state = state;
            _tokenService = tokenService;
            _validator = new GetBalanceMessageValidator(state);
        }

        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<IInboundMessage> messages)
        {
            await foreach (var message in messages.Where(m => m is GetBalanceMessage).Cast<GetBalanceMessage>())
            {
                BalanceMessage outputMessage = null;
                
                try
                {
                    var sessionLog = Log.ForContext("Session", message.UserId);
                    var result = await _validator.ValidateAsync(message);
                    
                    if (result.IsValid)
                    {
                        var address = await _state.GetAddress(message.UserId);
                        var senderBalance = await _tokenService.BalanceOfQueryAsync(address);
                        
                        outputMessage = new BalanceMessage(
                            UserId: message.UserId,
                            Amount: UnitConversion.Convert.FromWei(senderBalance));

                        sessionLog.Information("Processed {@Message}", message);
                    }
                    else
                    {
                        var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                        outputMessage = new BalanceMessage(
                            UserId: message.UserId,
                            Amount: default,
                            Errors: errors);

                        sessionLog.Information("Processed {@Message} with errors {@Errors}", message, errors);
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