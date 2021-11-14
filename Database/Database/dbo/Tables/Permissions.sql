CREATE TABLE [dbo].[Permissions] (
    [Id]          UNIQUEIDENTIFIER    NOT NULL,
    [Action]      NVARCHAR (42)       NOT NULL,
    [Value]       NVARCHAR (42)       NOT NULL,
    [ServiceName] NVARCHAR (42)       NOT NULL,
    [Description] NVARCHAR (402)       NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Permissions_Services] FOREIGN KEY ([ServiceName]) REFERENCES [dbo].[Services] ([Name])
);
