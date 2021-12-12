USE [MicroAC]
GO

DECLARE @User1   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User2   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User3   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User4   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User5   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User6   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User7   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User8   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User9   AS UNIQUEIDENTIFIER=NEWID();
DECLARE @User10  AS UNIQUEIDENTIFIER=NEWID();

DECLARE @Permission0  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission1 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission01 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission2  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission02 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission3  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission4  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission5  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission05 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission6  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission7  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission07 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission8  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission9  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission10 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission11 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission12 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission120 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission13 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission14 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission15 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission16 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission160 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission17 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission18 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission19 AS UNIQUEIDENTIFIER=NEWID();


INSERT INTO [dbo].[Organisations] ([Name]) VALUES
('MicroAC'),
('Analyticus'),
('IT Technologies'),
('Fine Solutions'),
('Co-Herent');

INSERT INTO [dbo].[Roles]([Name])VALUES
('Administrator'),
('Free User'),
('Client'),
('Consultant'),
('Manager'),
('Data Analyst');

INSERT INTO [dbo].[Users]([Id],[Name],[Surname],[Email],[Phone],[Organisation],[Blocked],[PasswordHash],[Salt])VALUES
(@User1,'admin'			,'admin' 		,'admin@admin.com'					,'+37065111111','MicroAC'			,0,NULL,NULL),
(@User2,'Antanas'		,'Antanaitis'	,'Antanas.Antanaitis@gmail.com'		,'+37065348586','MicroAC'			,0,NULL,NULL),
(@User3,'Jonas'			,'Jonaitis'		,'Jonas.Jonaitis@gmail.com'			,'+37065348586','MicroAC'			,0,NULL,NULL),
(@User4,'Kazimieras'	,'Antanaitis'	,'Kazimieras.antanaitis@gmail.com'	,'+37065348586','MicroAC'			,0,NULL,NULL),
(@User5,'Deimantė'		,'Deimantienė'	,'Deimantė.Deimantienė@gmail.com'	,'+37065348586','MicroAC'			,0,NULL,NULL),
(@User6,'Mantas'		,'Mantauskas'	,'Mantas.Mantauskas@gmail.com'		,'+37065348586','IT Technologies'	,0,NULL,NULL),
(@User7,'Justas'		,'Justauskas'	,'Justas.Justauskas@gmail.com'		,'+37065348586','Co-Herent'			,0,NULL,NULL),
(@User8,'Rolandas'		,'Rolandaitis'	,'Rolandas.Rolandaitis@gmail.com'	,'+37065348586','Fine Solutions'	,0,NULL,NULL),
(@User9,'Dovilė'		,'Dovilienė'	,'Dovilė.Dovilienė@gmail.com'		,'+37065348586','IT Technologies'	,0,NULL,NULL),
(@User10,'Dominykas'	,'Dominykaitis'	,'Dominykas.Dominykaitis@gmail.com'	,'+37065348586','Analyticus'		,0,NULL,NULL);

INSERT INTO [dbo].[Users_Roles]([User],[Role])VALUES
(@User1, 'Administrator'),
(@User2, 'Free User'),
(@User3, 'Client'),
(@User4, 'Consultant'),
(@User5, 'Manager'),
(@User6, 'Data Analyst'),
(@User7, 'Administrator'),
(@User8, 'Free User'),
(@User9, 'Client'),
(@User10, 'Consultant'),
(@User10, 'Administrator'),
(@User9, 'Free User'),
(@User8, 'Client'),
(@User7, 'Consultant'),
(@User6, 'Manager'),
(@User5, 'Data Analyst'),
(@User4, 'Administrator'),
(@User3, 'Free User'),
(@User2, 'Client'),
(@User1, 'Consultant');

INSERT INTO [dbo].[Services] ([Name]) VALUES
('MicroAC-Users'),
('Products'),
('Shipping'),
('Admin'),
('Customers'),
('Cart');

INSERT INTO [dbo].[Permissions]([Id],[ServiceName],[Action],[Value],[Description]) VALUES
(@Permission0 ,'Products'		,'All'		,'All'		,''),
(@Permission01,'Products'		,'View'		,'All'		,''),
(@Permission1 ,'Products'		,'View'		,'Active'	,''),
(@Permission2 ,'Products'		,'Update'	,'All'		,''),
(@Permission02,'Products'		,'Update'	,'Quantity'	,''),
(@Permission3 ,'Products'		,'Create'	,'All'		,''),
(@Permission4 ,'Products'		,'Delete'	,'Disabled'	,''),
(@Permission05,'Shipping'		,'View'		,'Orders'	,''),
(@Permission5 ,'Shipping'		,'Create'	,'Order'	,''),
(@Permission6 ,'Shipping'		,'Start'	,'Order'	,''),
(@Permission7 ,'Shipping'		,'Update'	,'Contacts'	,''),
(@Permission07,'Shipping'		,'Cancel'	,'Order'	,''),
(@Permission8 ,'MicroAC-Users'	,'Register'	,'User'		,''),
(@Permission9 ,'MicroAC-Users'	,'View'		,'Users'	,''),
(@Permission10,'MicroAC-Users'	,'Modify'	,'User'		,''),
(@Permission11,'MicroAC-Users'	,'Delete'	,'User'		,''),
(@Permission120,'Admin'			,'All'		,'All'		,''),
(@Permission12,'Admin'			,'Update'	,'Settings'	,''),
(@Permission13,'Admin'			,'Update'	,'Services'	,''),
(@Permission14,'Admin'			,'View'		,'Services'	,''),
(@Permission15,'Admin'			,'Edit'		,'Metadata'	,''),
(@Permission160,'Cart'			,'View'		,'All'		,''),
(@Permission16,'Cart'			,'View'		,'My-Items'	,''),
(@Permission17,'Cart'			,'Add'		,'My-Item'	,''),
(@Permission18,'Cart'			,'Remove'	,'My-Item'	,''),
(@Permission19,'Cart'			,'PlaceOrder','My-Cart'	,'');

INSERT INTO [dbo].[Roles_Permissions]([Role],[Permission])VALUES
('Administrator'	,@Permission0),
('Administrator'	,@Permission1),
('Data Analyst'		,@Permission01),
('Consultant'		,@Permission02),
('Consultant'		,@Permission5),
('Client'			,@Permission5),
('Free User'		,@Permission5),
('Data Analyst'		,@Permission05),
('Consultant'		,@Permission6),
('Consultant'		,@Permission7),
('Consultant'		,@Permission07),
('Administrator'	,@Permission8),
('Administrator'	,@Permission9),
('Administrator'	,@Permission10),
('Administrator'	,@Permission11),
('Administrator'	,@Permission120),
('Administrator'	,@Permission16),
('Consultant'		,@Permission160),
('Data Analyst'		,@Permission160),
('Free User'		,@Permission16),
('Free User'		,@Permission17),
('Free User'		,@Permission18),
('Free User'		,@Permission19),
('Client'			,@Permission16),
('Client'			,@Permission17),
('Client'			,@Permission18),
('Client'			,@Permission19);

GO