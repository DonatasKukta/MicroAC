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
DECLARE @Permission21 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission22 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission23 AS UNIQUEIDENTIFIER=NEWID();
DECLARE @Permission24 AS UNIQUEIDENTIFIER=NEWID();

INSERT INTO [dbo].[Organisations] ([Name]) VALUES
('MicroAC'),
('Analyticus'),
('IT Technologies'),
('Fine Solutions'),
('Co-Herent');

INSERT INTO [dbo].[Roles]([Name])VALUES
('View-Everything'),
('Update-Everything'),
('Delete-Everything'),
('Create-Everything');

INSERT INTO [dbo].[Users]([Id],[Name],[Surname],[Email],[Phone],[Organisation],[Blocked],[PasswordHash],[Salt])VALUES
(@User1, 'admin'		,'admin' 		,'everything@microac.com'				,'+37065111111','IT Technologies'	,0, 0xC71AC7AE8EC8E9684F28554561599E80F7DAA8E2629AADC2130D5350243A2E89B2C62C32790A0697946A36A944A200101F29AF5501451FFB1050A4068853C5F4, 0x46D086824097E4A395CFFF46699C73C4),
(@User2, 'Antanas'		,'Antanaitis'	,'view@microac.com'						,'+37065348586','Co-Herent'			,0, 0x8438BCA5CD89A3BBB720257264D2B0F41AC6514128A699CE557139C54EB39011CBC44C2153104EC9EFBF91E084D7CBC406005162D48EF8F77D560142D3A120DE, 0xA1CD1034135B4EA36F84A54ADF7A0EA0),
(@User3, 'Jonas'		,'Jonaitis'		,'update@microac.com'					,'+37065348586','Fine Solutions'	,0, 0x8F7A98B4E23AAF07B09660A02C7470B86C211315417DFAC2400E862BCEC7A5699F3455A0B4B6F0EDF536C2C7E27D6A45B64CA27C108B3FCCF2B338A0DE8EAAB7, 0x9CE3C1176CE48E824E0221BE0F3B5358),
(@User4, 'Kazimieras'	,'Antanaitis'	,'create@microac.com'					,'+37065348586','IT Technologies'	,0, 0xB6C7C7A835D7BD8712A0D43B16610AADBE211BEF92EDCCBA898C8B9F164F3EC816B6DB6D02D699BCB649203D93CFEF5B1918296596C862AB1A7BE6A0958C8BC8, 0xF0A328184BE991EB81E312BCF19AC41A),
(@User5, 'Deimantė'		,'Deimantienė'	,'delete@microac.com'					,'+37065348586','Analyticus'		,0, 0xA7AEA6A8A24915BD84CA8F71F7181B16DC20D069A0DA9183618A95328693C80DC366D06134F0E87E7E891DFB2A50B282AFD9AFF5F8E41AAA4BF9A36E4F11E3B6, 0xC15EA986DA5B57BD3F8EE610398F9334),
(@User6, 'Mantas'		,'Mantauskas'	,'Mantas.Mantauskas@microac.com'		,'+37065348586','IT Technologies'	,0, 0x166626CA623C7F22CF9D68A21375B22D773FDB956A1CA6AF5B90700BE4ED916203E0CB9EADE9A8C79619640F48086E20F6B07D0F4E890061F8DB04BB15B1DD60, 0x304D5A3C52F655B351D69589D1D290B4),
(@User7, 'Justas'		,'Justauskas'	,'Justas.Justauskas@microac.com'		,'+37065348586','Co-Herent'			,0, 0xF1F0A54073F18EBD99E9AF9D360AF9F4D3AEEC7551CA332E8E94470DD995B844B50B8BA16F0AD4B146B1C65E548E2B7BA0AC789060393FAD0F6C3966EAEBC24F, 0x3E07FDB192CD7D944B6BD9BD0380B1F2),
(@User8, 'Rolandas'		,'Rolandaitis'	,'Rolandas.Rolandaitis@microac.com'		,'+37065348586','Fine Solutions'	,0, 0xC21463762A85B24648454B69158BE45286B2D86ECC1867D8440E7FF8E2A56BFD4C839666AF400C21F2E4FB971C487D33A8BA6267E7C0B9DCA3B1B2F858117B37, 0x4FDE8BA43F87C7CA08F63404C6CEEC6D),
(@User9, 'Dovilė'		,'Dovilienė'	,'Dovilė.Dovilienė@microac.com'			,'+37065348586','IT Technologies'	,0, 0x85A75F7C0382185A6DBF0C89276FA0A00F311D3B94D70CAA089696024E09613CF82BC2324624F3BF200D81B51836E47325FADA0823FD3C39AADFF4BB71FB2FBB, 0x25A33AF246EC74B73F32A89B9879CE7D),
(@User10,'Dominykas'	,'Dominykaitis'	,'Dominykas.Dominykaitis@microac.com'	,'+37065348586','Analyticus'		,0, 0x173DAD564F87C2505E22D99610725E62958DD90118F7A234A3B9BA83388A353CB92AC555AB8516E8AA5D4A18274E57C2FC6D9B98C5036326B27455C1B2D56543, 0x7F6BBFD1AE7F5F6C3C28D47A96E0AD70);

INSERT INTO [dbo].[Users_Roles]([User],[Role])VALUES
(@User1, 'View-Everything'	),
(@User1, 'Update-Everything'),
(@User1, 'Delete-Everything'),
(@User1, 'Create-Everything'),
(@User2, 'View-Everything'  ),
(@User3, 'Update-Everything'),
(@User4, 'Delete-Everything'),
(@User5, 'Create-Everything');

INSERT INTO [dbo].[Services] ([Name]) VALUES
('Carts'),
('Products'),
('Shipments'),
('Orders');

INSERT INTO [dbo].[Permissions]([Id],[ServiceName],[Action],[Value],[Description]) VALUES
(@Permission1	,'Products'	,'View'	  ,'One'			,''),
(@Permission2	,'Products'	,'View'	  ,'All'			,''),
(@Permission3	,'Products'	,'Create' ,''				,''),
(@Permission4	,'Products'	,'Update' ,''				,''),
(@Permission5	,'Products'	,'Delete' ,''				,''),
(@Permission6	,'Products'	,'Delete' ,''				,''),
(@Permission7	,'Carts'	,'Create' ,''				,''),
(@Permission8	,'Carts'	,'Get'	  ,'Self'			,''),
(@Permission9	,'Carts'	,'Update' ,'Self'			,''),
(@Permission10	,'Carts'	,'Delete' ,'Cart'			,''),
(@Permission11	,'Carts'	,'Delete' ,'CartItem'		,''),
(@Permission12	,'Shipments','View'	  ,'One'			,''),
(@Permission13	,'Shipments','View'	  ,'All'			,''),
(@Permission14	,'Shipments','Create' ,'' 				,''),
(@Permission15	,'Shipments','Update' ,'' 				,''),
(@Permission16	,'Shipments','Delete' ,'' 				,''),
(@Permission17	,'Orders'	,'View'	  ,'One'			,''),
(@Permission18	,'Orders'	,'View'	  ,'All'			,''),
(@Permission19	,'Orders'	,'Create' ,''				,''),
(@Permission21	,'Orders'	,'Submit' ,'PaymentDetails'	,''),
(@Permission22	,'Orders'	,'Submit' ,'ShipmentDetails',''),
(@Permission23	,'Orders'	,'Submit' ,'Order'			,''),
(@Permission24	,'Orders'	,'Delete' ,'Self'			,'');

INSERT INTO [dbo].[Roles_Permissions]([Role],[Permission])VALUES
('View-Everything'		,@Permission1	),
('View-Everything'		,@Permission2	),
('Create-Everything'	,@Permission3	),
('Update-Everything'	,@Permission4	),
('Delete-Everything'	,@Permission5	),
('Delete-Everything'	,@Permission6	),
('Create-Everything'	,@Permission7	),
('View-Everything'		,@Permission8	),
('Update-Everything'	,@Permission9	),
('Delete-Everything'	,@Permission10	),
('Delete-Everything'	,@Permission11	),
('View-Everything'		,@Permission12	),
('View-Everything'		,@Permission13	),
('Create-Everything'	,@Permission14	),
('Update-Everything'	,@Permission15	),
('Delete-Everything'	,@Permission16	),
('View-Everything'		,@Permission17	),
('View-Everything'		,@Permission18	),
('Create-Everything'	,@Permission19	),
('Update-Everything'	,@Permission21	),
('Update-Everything'	,@Permission22	),
('Update-Everything'	,@Permission23	),
('Delete-Everything'	,@Permission24	);

GO