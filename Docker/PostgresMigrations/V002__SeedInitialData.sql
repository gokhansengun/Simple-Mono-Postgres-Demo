DELETE FROM "asp_net_user_roles";
DELETE FROM "asp_net_roles";
DELETE FROM "asp_net_users";

INSERT INTO "asp_net_roles" ("id", "description", "name") VALUES ('4c66ead4-a39a-4751-a4d5-8fc01a96e03a', 'Plain vanilla User', 'Users');
INSERT INTO "asp_net_roles" ("id", "description", "name") VALUES ('89cd6a60-b149-4051-adcd-ba71f41b47ff', 'All access pass', 'Admin');

INSERT INTO "asp_net_users" ("id", "email", "email_confirmed", "password_hash", "security_stamp", "phone_number", "phone_number_confirmed", "two_factor_enabled", "lockout_end_date_utc", "lockout_enabled", "access_failed_count", "user_name") VALUES ('1878f71b-2471-481c-abe4-d09647a78e73', 'vanillaUser@example.com', false, 'ADGZ+8ISYxITMi/4/PkvlLqE59T8L/dLbWQ81rIqtDzuRTPUyRz7iJGJUipRRCvVLg==', '34a98d49-867e-4f59-83d9-e259baa39d1a', NULL, false, false, NULL, false, 0, 'vanillaUser@example.com');
INSERT INTO "asp_net_users" ("id", "email", "email_confirmed", "password_hash", "security_stamp", "phone_number", "phone_number_confirmed", "two_factor_enabled", "lockout_end_date_utc", "lockout_enabled", "access_failed_count", "user_name") VALUES ('40180e73-de4d-4eb5-9019-040a92ebb3b0', 'admin@example.com', false, 'AB4O33Fd4wcrClvDl+hatjrctngu2m0j9ZyRIDgouBYEBWZWuzDpoUc0J6Od7LLj1Q==', 'c2d5895a-5b11-4839-a7c7-9137d4018c5b', NULL, false, false, NULL, false, 0, 'admin@example.com');

INSERT INTO "asp_net_user_roles" ("user_id", "role_id") VALUES ('1878f71b-2471-481c-abe4-d09647a78e73', '4c66ead4-a39a-4751-a4d5-8fc01a96e03a');
INSERT INTO "asp_net_user_roles" ("user_id", "role_id") VALUES ('40180e73-de4d-4eb5-9019-040a92ebb3b0', '89cd6a60-b149-4051-adcd-ba71f41b47ff');

