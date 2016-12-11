var cutil = require('../../../util/cutil');
var player = require('../../../models/player');

module.exports = function(app) {
	return new ChatRemote(app);
};

var ChatRemote = function(app) {
	this.app = app;
	this.channelService = app.get('channelService');
};

/**
 * Add user into chat channel.
 *
 * @param {String} uid unique id for user
 * @param {String} sid server id
 * @param {String} name channel name
 * @param {boolean} flag channel parameter
 *
 */
ChatRemote.prototype.add = function(uid, sid, name, posx, posy,flag, cb) {
	var channel = this.channelService.getChannel(name, flag);
	var username = uid.split('*')[0];
	var param = {
		route: 'onAdd',
		user: username,
		posx: posx,
		posy: posy
	};
	channel.pushMessage(param); // push message to client(should use onAdd to receive the message)

	//添加用户
	player.addUser({uid:uid,name:username,posx:posx,posy:posy});

	if( !! channel) {
		channel.add(uid, sid);
	}

	cb(this.get(name,flag));
};

/**
 * Get user from chat channel.
 *
 * @param {Object} opts parameters for request
 * @param {String} name channel name
 * @param {boolean} flag channel parameter
 * @return {Array} users uids in channel
 *
 */
ChatRemote.prototype.get = function(name,flag) {
	//var users = [];
	//var channel = this.channelService.getChannel(name, flag);
	//if( !! channel) {
	//	users = channel.getMembers();
	//}
    //
	//for(var i = 0; i < users.length; i++) {
	//	users[i] = users[i].split('*')[0];
	//}
	//return users;

	var rs = player.getUsers();
	return rs;
};

/**
 * Kick user out chat channel.
 *
 * @param {String} uid unique id for user
 * @param {String} sid server id
 * @param {String} name channel name
 *
 */
ChatRemote.prototype.kick = function(uid, sid, name, cb) {
	var channel = this.channelService.getChannel(name, false);
	// leave channel
	if( !! channel) {
		channel.leave(uid, sid);
	}
	var username = uid.split('*')[0];

	// 删除
	player.rmUser(uid);

	var param = {
		route: 'onLeave',
		user: username
	};
	channel.pushMessage(param);
	cb();
};