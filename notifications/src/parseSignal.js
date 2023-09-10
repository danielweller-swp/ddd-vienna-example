function parseSignal(signalMessage) {
  const latitude = signalMessage.Latitude
  const longitude = signalMessage.Longitude
  const timestamp = Date.parse(signalMessage.Timestamp)

  if (signalMessage.ValidationResult.Case == 'Valid') {
    return {
      isValid: true,
      latitude,
      longitude,
      timestamp
    }
  } else {
    return {
      isValid: false,
      latitude,
      longitude,
      timestamp,
      error: signalMessage.ValidationResult.Fields[0]
    }
  }
}

module.exports = { parseSignal }