var util = require('util');

var cutil = module.exports;

//generate a random number between min and max
cutil.rand = function (min, max) {
    var n = max - min;
    return min + Math.round(Math.random() * n);
};
