serialSearch = (keywords, cb) ->
  out = []
  for k,i in keywords
    await search k, defer out[i]
  cb out 