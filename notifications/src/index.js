const { parseSignal } = require('./parseSignal')

const express = require('express')

function extractSignalMessage(postBody) {
  return JSON.parse(Buffer.from(postBody.message.data, 'base64').toString('utf8'))
}

function notifyCustomer(invalidSignal) {
  console.log(`Invalid signal ${JSON.stringify(invalidSignal)} observed, notifying customer.`)
}

function invalidSignalNotificationHandler(req, res) {
  const signal = parseSignal(extractSignalMessage(req.body))
  if (signal.isValid) {
    console.log('Valid signal observed; doing nothing.')
  } else {
    notifyCustomer(signal)
  }
  res.sendStatus(200)
}

const app = express()
const port = 8080

app.use(express.json())

app.post('/', invalidSignalNotificationHandler)

app.listen(port, () => {
  console.log(`invalidSignalNotificationHandler listening on port ${port}`)
})
