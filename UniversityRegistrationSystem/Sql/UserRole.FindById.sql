/*
 * UserRole.FindById.sql
 */

SELECT 
	user_id AS UserId, 
	role_id AS RoleId 
FROM asp_net_user_roles 
WHERE role_id=@roleId