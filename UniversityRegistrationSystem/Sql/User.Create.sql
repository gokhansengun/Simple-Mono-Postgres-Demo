/*
 * User.Create.sql
 */

INSERT INTO asp_net_users(
	id, 
	user_name, 
	email, 
	first_name, 
	last_name, 
	phone_number, 
	password_hash, 
	security_stamp, 
	email_confirmed, 
	phone_number_confirmed)
VALUES(
	@id, 
	lower(@userName), 
	lower(@email), 
	@firstName, 
	@lastName, 
	@phoneNumber,
	@passwordHash, 
	@securityStamp, 
	@emailConfirmed, 
	@phoneNumberConfirmed)