CREATE TABLE [dbo].[Users] (
    [Id]           UNIQUEIDENTIFIER    NOT NULL,
    [Name]         NVARCHAR (42)       NOT NULL,
    [Surname]      NVARCHAR (42)       NOT NULL,
    [Email]        NVARCHAR (82)       NULL,
    [Phone]        NVARCHAR (22)       NOT NULL,
    [Organisation] NVARCHAR (42)       NULL,
    [Role]         NVARCHAR (42)       NOT NULL,
    [Blocked]      BIT CONSTRAINT [DF_Users_blocked] DEFAULT ((0)) NOT NULL,
    /* TODO: Set to NOT NULL */
    [PasswordHash] BINARY (64)       NULL,
    [Salt]         BINARY (16)       NULL,
    CONSTRAINT [FK_Users_Organisations] FOREIGN KEY ([Organisation]) REFERENCES [dbo].[Organisations] ([Name]),
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([Role]) REFERENCES [dbo].[Roles] ([Name])
);
