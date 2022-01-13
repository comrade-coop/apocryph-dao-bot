import { create } from 'ipfs-core'

export default {
  install: (app, options) => {
    app.config.globalProperties.$ipfs = create(options)
  }
}

// how to specify ipfs node: https://stackoverflow.com/questions/63696779/how-to-start-ipfs-in-browser-by-using-only-dht-webrtc-peers
