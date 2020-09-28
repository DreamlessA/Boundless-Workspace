const http = require('http');

const HOST = 'localhost';
const PORT = '3000';

const add = ({ path, type, data }) => {
  const body = JSON.stringify({ path, type, data });

  const req = http.request({
    host: HOST,
    port: PORT,
    path: '/add',
    method: 'POST',
    headers: {
      "Content-Type": "application/json",
      "Content-Length": Buffer.byteLength(body)
    }
  }, res => {
    res.setEncoding('utf8');
    res.on('data', chunk => {
      // console.log(JSON.parse(chunk));
    });
  });
  req.end(body);
};

module.exports = {
  add
};
