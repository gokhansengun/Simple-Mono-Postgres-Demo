/*
 * User.FindById.sql
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
	email_confirmed AS EmailConfirmed, 
	phone_number_confirmed AS PhoneNumberConfirmed 
FROM asp_net_users 
WHERE id = @id