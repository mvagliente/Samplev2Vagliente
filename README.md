# Code Review 
# Backend
Add auth controler to login user and get the token (berear) and refreshtoken (to avoid users need to re login when expires)

Add crud services that use all that instances that inherit from Entity class

Add Repository Generic that use all that instances that inherit from Entity class.

Whit CrudService and Repository we can handle all the model classes to avoid recreate services and respository for each model entity.

Add logger on the controllers and return code 500 in case of uncontrolled error.

Add initial load data of database on seeding method working with EF core.

Using identity for the users.roles,etc so the services and classes of users was replaced by identityuser identityrol, etc.

Add configuration on swagger for Bearer token if you want to get information from office or users need to send the token otherwise its going to respond with unauthorized code 401.

Add dependency injection for crudservice and repository and defaultidentity config.

Unit testing is missing on the application should have the unit testing  from each controller and services to get un code coverage of at least 90%,

change the way of how it used the context, whit a context factory where only the repository called.

# UI
Should  have  the login to get the token

Should have interceptor to attach the login and resolve the refreshTOKEN parameter on case of token expires and avoid to re-login for the users

Should have unit testing


# Project setup
## netcore backend

run 

PM> update-database 

## webapp

npm install

npm run serve

