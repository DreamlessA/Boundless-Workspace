'use strict';
module.exports = (app) => {
  app.route('/test')
    .get((req, res) => {
      console.log(req);
      res.json({message: "heyyy"});
    });

  // todoList Routes
  app.route('/add')
    .post((req, res) => {
      const queue = req.app.get('queue');
      // console.log(queue);
      // console.log(req.body);
      queue.push(req.body);
      // console.log(queue);
      res.json({message: "got it"});
    });

  app.route('/remove')
    .post((req, res) => {
      const next = req.app.get('queue').shift();
      if (next === undefined) {
        res.json({});
      } else {
        res.json({
          next: next
        });
      }
    });

  app.route('/debug')
    .post((req, res) => {
      res.json({queue: req.app.get('queue').map(x => {
        const { data, ...rest } = x;
        // Don't return the data.
        return rest;
      })});
    });
};
