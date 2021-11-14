CREATE TABLE [dbo].[Role_Permissions] (
    [Role]       NVARCHAR (42)    NOT NULL,
    [Permission] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Role_Permissions_Permissions] FOREIGN KEY ([Permission]) REFERENCES [dbo].[Permissions] ([Id]),
    CONSTRAINT [FK_Role_Permissions_Roles] FOREIGN KEY ([Role]) REFERENCES [dbo].[Roles] ([Name])
);
