CREATE TABLE [dbo].[Roles_Permissions] (
    [Role]       NVARCHAR (42)    NOT NULL,
    [Permission] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Roles_Permissions] PRIMARY KEY CLUSTERED ([Role], [Permission] ASC),
    CONSTRAINT [FK_Roles_Permissions_Permissions] FOREIGN KEY ([Permission]) REFERENCES [dbo].[Permissions] ([Id]),
    CONSTRAINT [FK_Roles_Permissions_Roles] FOREIGN KEY ([Role]) REFERENCES [dbo].[Roles] ([Name])
);
