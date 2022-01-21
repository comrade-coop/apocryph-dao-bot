import { create } from 'ipfs-http-client'

export default {
  install: (app, options) => {
    app.config.globalProperties.$ipfs = create(options)
  }
}