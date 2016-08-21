/*
 * User.FindByName.sql
 */

SELECT 
	user_name AS UserName, 
	password_hash AS PasswordHash, 
	security_stamp AS SecurityStamp, 
	phone_number AS PhoneNumber, 
	email AS Email, 
	first_name AS FirstName, 
	last_name AS LastName, 
	id AS Id, 
	email_confirmed as EmailConfirmed, 
	phone_number_confirmed as PhoneNumberConfirmed 
FROM asp_net_users
WHERE lower(user_name) = lower(@userName)