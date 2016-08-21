/*
 * UserRole.FindByName.sql
 */

SELECT 
	id AS Id, 
	name AS Name, 
	description AS Description 
FROM asp_net_roles
WHERE name=@Name