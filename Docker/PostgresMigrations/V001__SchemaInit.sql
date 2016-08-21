BEGIN;

CREATE TABLE "public"."asp_net_roles"( 
	"id" varchar(128) NOT NULL,
	"description" text,
	"name" varchar(256) NOT NULL);

CREATE TABLE "public"."asp_net_user_claims"( 
	"id" int NOT NULL,
	"user_id" varchar(128) NOT NULL,
	"claim_type" text,
	"claim_value" text);

CREATE TABLE "public"."asp_net_user_logins"( 
	"login_provider" varchar(128) NOT NULL,
	"provider_key" varchar(128) NOT NULL,
	"user_id" varchar(128) NOT NULL);

CREATE TABLE "public"."asp_net_user_roles"( 
	"user_id" varchar(128) NOT NULL,
	"role_id" varchar(128) NOT NULL);

CREATE TABLE "public"."asp_net_users"( 
	"id" varchar(128) NOT NULL,
	"email" varchar(256),
	"email_confirmed" boolean NOT NULL,
	"password_hash" text,
	"security_stamp" text,
	"phone_number" text,
	"phone_number_confirmed" boolean NOT NULL,
	"two_factor_enabled" boolean,
	"lockout_end_date_utc" timestamp,
	"lockout_enabled" boolean,
	"access_failed_count" int,
	"user_name" varchar(256) NOT NULL,
	"first_name" varchar(256),
	"last_name" varchar(256));

ALTER TABLE "public"."asp_net_roles" ADD CHECK (char_length("id") <= 128);
ALTER TABLE "public"."asp_net_roles" ADD CHECK (char_length("name") <= 256);
ALTER TABLE "public"."asp_net_user_claims" ADD CHECK (char_length("user_id") <= 128);
ALTER TABLE "public"."asp_net_user_logins" ADD CHECK (char_length("login_provider") <= 128);
ALTER TABLE "public"."asp_net_user_logins" ADD CHECK (char_length("provider_key") <= 128);
ALTER TABLE "public"."asp_net_user_logins" ADD CHECK (char_length("user_id") <= 128);
ALTER TABLE "public"."asp_net_user_roles" ADD CHECK (char_length("user_id") <= 128);
ALTER TABLE "public"."asp_net_user_roles" ADD CHECK (char_length("role_id") <= 128);
ALTER TABLE "public"."asp_net_users" ADD CHECK (char_length("id") <= 128);
ALTER TABLE "public"."asp_net_users" ADD CHECK (char_length("email") <= 256);
ALTER TABLE "public"."asp_net_users" ADD CHECK (char_length("user_name") <= 256);
ALTER TABLE "public"."asp_net_users" ADD CHECK (char_length("first_name") <= 256);
ALTER TABLE "public"."asp_net_users" ADD CHECK (char_length("last_name") <= 256);

COMMIT;

CREATE SEQUENCE "public"."asp_net_user_claims_id_seq" INCREMENT BY 1 MINVALUE 1 START WITH 1 OWNED BY "public"."asp_net_user_claims"."id";
ALTER TABLE "public"."asp_net_roles" ADD CONSTRAINT "pk_dbo.asp_net_roles" PRIMARY KEY ("id");
ALTER TABLE "public"."asp_net_user_claims" ADD CONSTRAINT "pk_dbo.asp_net_user_claims" PRIMARY KEY ("id");
ALTER TABLE "public"."asp_net_user_logins" ADD CONSTRAINT "pk_dbo.asp_net_user_logins" PRIMARY KEY ("login_provider","provider_key","user_id");
ALTER TABLE "public"."asp_net_user_roles" ADD CONSTRAINT "pk_dbo.asp_net_user_roles" PRIMARY KEY ("user_id","role_id");
ALTER TABLE "public"."asp_net_users" ADD CONSTRAINT "pk_dbo.asp_net_users" PRIMARY KEY ("id");
CREATE UNIQUE INDEX "role_name_index" ON "public"."asp_net_roles" ("name" ASC);
CREATE INDEX "ix_user_id2pgi0" ON "public"."asp_net_user_claims" ("user_id" ASC);
CREATE INDEX "ix_user_id2pgi1" ON "public"."asp_net_user_logins" ("user_id" ASC);
CREATE INDEX "ix_role_id" ON "public"."asp_net_user_roles" ("role_id" ASC);
CREATE INDEX "ix_user_id" ON "public"."asp_net_user_roles" ("user_id" ASC);
CREATE UNIQUE INDEX "user_name_index" ON "public"."asp_net_users" ("user_name" ASC);
ALTER TABLE "public"."asp_net_user_claims" ADD CONSTRAINT "fk_dbo.asp_net_user_claims_dbo.asp_net_users_user_id" FOREIGN KEY ("user_id") REFERENCES "public"."asp_net_users" ( "id") ON DELETE CASCADE;
ALTER TABLE "public"."asp_net_user_logins" ADD CONSTRAINT "fk_dbo.asp_net_user_logins.asp_net_users_user_id" FOREIGN KEY ("user_id") REFERENCES "public"."asp_net_users" ( "id") ON DELETE CASCADE;
ALTER TABLE "public"."asp_net_user_roles" ADD CONSTRAINT "fk_dbo.asp_net_user_roles_dbo.asp_net_roles_role_id" FOREIGN KEY ("role_id") REFERENCES "public"."asp_net_roles" ( "id") ON DELETE CASCADE;
ALTER TABLE "public"."asp_net_user_roles" ADD CONSTRAINT "fk_dbo.asp_net_user_roles_dbo.asp_net_users_user_id" FOREIGN KEY ("user_id") REFERENCES "public"."asp_net_users" ( "id") ON DELETE CASCADE;
ALTER TABLE "public"."asp_net_user_claims" ALTER COLUMN "id" SET DEFAULT nextval('"public"."asp_net_user_claims_id_seq"');
select setval('"public"."asp_net_user_claims_id_seq"',(select max("id") from "public"."asp_net_user_claims")::bigint);
