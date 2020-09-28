var express = require('express'),
  app = express(),
  port = process.env.PORT || 3000,
  bodyParser = require('body-parser');


app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json({ limit: '50mb' }));


var routes = require('./routes'); //importing route
routes(app); //register the route

app.set('queue', []);

app.listen(port);


console.log('Boundless Workspace API server started on: ' + port);
