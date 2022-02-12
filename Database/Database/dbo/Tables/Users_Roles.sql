CREATE TABLE [dbo].[Users_Roles]
(
    [User]       UNIQUEIDENTIFIER NOT NULL,
    [Role]       NVARCHAR (42)    NOT NULL,
    CONSTRAINT [PK_Users_Roles] PRIMARY KEY CLUSTERED ([User], [Role] ASC),
    CONSTRAINT [FK_Users_Roles_Users] FOREIGN KEY ([User]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Users_Roles_Roles] FOREIGN KEY ([Role]) REFERENCES [dbo].[Roles] ([Name])
)
