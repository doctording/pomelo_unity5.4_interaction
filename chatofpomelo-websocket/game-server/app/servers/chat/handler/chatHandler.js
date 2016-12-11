var player = require('../../../models/player');


var chatRemote = require('../remote/chatRemote');

module.exports = function(app) {
	return new Handler(app);
};

var Handler = function(app) {
	this.app = app;
};

var handler = Handler.prototype;

/**
 * Send messages to users
 *
 * @param {Object} msg message from client
 * @param {Object} session
 * @param  {Function} next next stemp callback
 *
 */
handler.send = function(msg, session, next) {
	var rid = session.get('rid');
	var username = session.uid.split('*')[0];
	var channelService = this.app.get('channelService');
	var param = {
		msg: msg.content,
		from: username,
		target: msg.target
	};
	channel = channelService.getChannel(rid, false);

	//the target is all users
	if(msg.target == '*') {
		channel.pushMessage('onChat', param);
	}
	//the target is specific user
	else {
		var tuid = msg.target + '*' + rid;
		var tsid = channel.getMember(tuid)['sid'];
		channelService.pushMessageByUids('onChat', param, [{
			uid: tuid,
			sid: tsid
		}]);
	}
	next(null, {
		route: msg.route
	});
};

handler.move = function(msg, session, next) {
	console.log("--------move-----------");
	var rid = session.get('rid');
	var username = session.uid.split('*')[0];
	var channelService = this.app.get('channelService');

	//player.addUser({uid:uid,name:username,posx:posx,posy:posy});
	var posx = msg.posx;
	var posy = msg.posy;

	player.updateUser({uid:session.uid,username:username,posx:posx,posy:posy});

	var param = {
		posx: msg.posx,
		posy: msg.posy,
		from: username,
		target: msg.target
	};
	channel = channelService.getChannel(rid, false);

	//the target is all users
	if(msg.target == '*') {
		channel.pushMessage('onMove', param);
	}
	//the target is specific user
	else {
		var tuid = msg.target + '*' + rid;
		var tsid = channel.getMember(tuid)['sid'];
		channelService.pushMessageByUids('onMove', param, [{
			uid: tuid,
			sid: tsid
		}]);
	}
	next(null, {
		route: msg.route
	});
};