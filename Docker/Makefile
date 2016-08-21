.PHONY: build flyway-migrator app clean ps

build:
	docker-compose up build

flyway-migrator:
	docker-compose up -d db
	docker-compose up flyway-migrator

app: build flyway-migrator
	docker-compose up -d app

db:
	docker-compose up -d db

clean:
	docker-compose down -v
	docker rmi $$(docker images | grep "^urs-" | awk '{print $$3}')
	
ps:
	docker-compose ps 
