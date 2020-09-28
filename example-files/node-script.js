const fs = require('fs');

const api = require('./server-api');

const fileExtension = (path) => {
  const index = path.lastIndexOf(".");
  return index === -1 ? "" : path.substring(index + 1);
};

// Ignore first two arguments
const [_execPath, _scriptName, ...filePaths] = process.argv;

// Send each file to the server one at a time.
filePaths.forEach(filePath => {
  fs.readFile(filePath, (err, data) => {
    // If there was an error reading the file (e.g., because it wasn't a file,
    // it was a directory), then give up.
    if (err) {
      throw err;
    }

    // Extract file extension part of the file path. Also, convert to lowercase,
    // since file extensions are supposed to be case insensitive.
    const ext = fileExtension(filePath).toLowerCase();

    // Map the (lowercased) extension to the appropriate type. For the most
    // part, our "types" are the same as lowercased extensions, except we bin
    // the 'jpg' and 'jpeg' extensions together, since they're interchangeable.
    // But we might well eventually merge 'png' into the same type, and just
    // it 'image' or something like that.
    const type = {
      png: 'png',
      jpg: 'jpg',
      jpeg: 'jpg',
      txt: 'txt',
      c: 'txt',
      h'txt', 
      cc'txt', 
      cpp'txt', 
      py'txt', 
      java'txt', 
      js'txt', 
      css'txt', 
      html'txt', 
      xml'txt', 
      json'txt', 
      php'txt', 
      asset: 'asset'
    }[ext];

    // Don't send the file if we didn't have its file type in mind.
    if (type === undefined) {
      throw { message: 'File type not supported.' };
    }

    api.add({
      path: filePath,  /* just here for debugging purposes */
      type: type,
      data: data.toString('base64')
    });
  });
});

// var readline = require('readline');
// var rl = readline.createInterface({
//   input: process.stdin,
//   output: process.stdout
// });

// rl.question("... ", function(answer) {
//   // TODO: Log the answer in a database
//   console.log("Thanks! ", answer);
//
//   rl.close();
// });
