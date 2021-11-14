USE [MicroAC]
GO

INSERT INTO [dbo].[Organisations] ([Name]) VALUES
('MicroAC'),
('Analyticus'),
('IT Technologies'),
('Fine Solutions'),
('Co-Herent');

INSERT INTO [dbo].[Roles]([name])VALUES
('Administrator'),
('Free User'),
('Client'),
('Wharehouse Worker'),
('Manager'),
('Data Analyst');

INSERT INTO [dbo].[Users]([Id],[Name],[Surname],[Email],[Phone],[Organisation],[Role],[Blocked],[PasswordHash],[Salt])VALUES
(NEWID(),'admin','admin','admin@admin.com','+37065111111','MicroAC','Administrator',0,NULL,NULL),
(NEWID(),'Antanas','Antanaitis','Antanas.Antanaitis@gmail.com','+37065348586','MicroAC','Manager',0,NULL,NULL),
(NEWID(),'Jonas','Jonaitis','Jonas.Jonaitis@gmail.com','+37065348586','MicroAC','Manager',0,NULL,NULL),
(NEWID(),'Kazimieras','Antanaitis','Kazimieras.antanaitis@gmail.com','+37065348586','MicroAC','Data Analyst',0,NULL,NULL),
(NEWID(),'Deimantė','Deimantienė','Deimantė.Deimantienė@gmail.com','+37065348586','MicroAC','Data Analyst',0,NULL,NULL),
(NEWID(),'Mantas','Mantauskas','Mantas.Mantauskas@gmail.com','+37065348586','MicroAC','Wharehouse Worker',0,NULL,NULL),
(NEWID(),'Justas','Justauskas','Justas.Justauskas@gmail.com','+37065348586','Co-Herent','Client',0,NULL,NULL),
(NEWID(),'Rolandas','Rolandaitis','Rolandas.Rolandaitis@gmail.com','+37065348586','Fine Solutions','Client',0,NULL,NULL),
(NEWID(),'Dovilė','Dovilienė','Dovilė.Dovilienė@gmail.com','+37065348586','IT Technologies','Free User',0,NULL,NULL),
(NEWID(),'Dominykas','Dominykaitis','Dominykas.Dominykaitis@gmail.com','+37065348586','Analyticus','Free User',0,NULL,NULL);

INSERT INTO [dbo].[Services] ([Name]) VALUES
('MicroAC-Users'),
('Products'),
('Shipping'),
('Admin'),
('Customers'),
('Cart'),
('Wharehouse');

DECLARE @Permission1  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission2  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission3  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission4  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission5  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission6  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission7  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission8  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission9  AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission10 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission11 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission12 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission13 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission14 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission15 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission16 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission17 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission18 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission19 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission20 AS UNIQUEIDENTIFIER=NEWID();


INSERT INTO [dbo].[Permissions]([Id],[ServiceName],[Action],[Value],[Description]) VALUES
(@Permission1 ,'Products'		,'View'		,'All'		,''),
(@Permission2 ,'Products'		,'Update'	,'All'		,''),
(@Permission3 ,'Products'		,'Create'	,'All'		,''),
(@Permission4 ,'Products'		,'Delete'	,'All'		,''),
(@Permission5 ,'Shipping'		,'Create'	,'Order'	,''),
(@Permission6 ,'Shipping'		,'Start'	,'Order'	,''),
(@Permission7 ,'Shipping'		,'Update'	,'Contacts'	,''),
(@Permission8 ,'MicroAC-Users'	,'Register'	,'User'		,''),
(@Permission9 ,'MicroAC-Users'	,'View'		,'Users'	,''),
(@Permission10,'MicroAC-Users'	,'Modify'	,'User'		,''),
(@Permission11,'MicroAC-Users'	,'Delete'	,'User'		,''),
(@Permission12,'Admin'			,'Update'	,'Settings'	,''),
(@Permission13,'Admin'			,'Update'	,'Services'	,''),
(@Permission14,'Admin'			,'View'		,'Services'	,''),
(@Permission15,'Admin'			,'Edit'		,'Metadata'	,''),
(@Permission16,'Cart'			,'View'		,'My-Items'	,''),
(@Permission17,'Cart'			,'Add'		,'My-Item'	,''),
(@Permission18,'Cart'			,'Remove'	,'My-Item'	,''),
(@Permission19,'Wharehouse'		,'Add'		,'Item'		,''),
(@Permission20,'Wharehouse'		,'Remove'	,'Item'		,'');


/*
INSERT INTO [dbo].[Role_Permissions]([Role],[Permission],[Organisation])VALUES
('','',''),
('','',''),
('','',''),
('','',''),
('','',''),
('','',''),
('','',''),
('','',''),
('','','');
*/

GO