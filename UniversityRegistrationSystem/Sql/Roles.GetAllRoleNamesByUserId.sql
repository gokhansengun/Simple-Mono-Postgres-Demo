/*
 * Roles.GetAllRoleNamesByUserId.sql
 */

 SELECT 
	r.name AS Name
FROM asp_net_roles r
INNER JOIN asp_net_user_roles ur ON ur.role_id = r.Id
INNER JOIN asp_net_users u ON ur.user_id = u.Id
WHERE 
	u.id = @id