#!/bin/bash

# Exit immediately if a command exits with a non-zero status.
set -e

host="$1"
username="$2" ## the username in db
database="$3" ## the database name
pgpass="$4" ## the password in db
shift

until PGPASSWORD=$pgpass psql -h "$host" -U "$username" -c '\l'; do
  >&2 echo "Postgres is unavailable - sleeping"
  sleep 1
done

>&2 echo "Postgres is up - starting the Flyway"

# clean the database
>&2 echo "Flyway is cleaning the database"
flyway -url=jdbc:postgresql://"$host":5432/"$database" -user="$username" -password="$pgpass" clean

# migrate the database
>&2 echo "Flyway is now running pending migrations -if any- on the database"
flyway -url=jdbc:postgresql://"$host":5432/"$database" -user="$username" -password="$pgpass" migrate

