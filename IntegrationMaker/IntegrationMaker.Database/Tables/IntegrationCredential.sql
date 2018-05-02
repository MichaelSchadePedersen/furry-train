CREATE TABLE [dbo].[IntegrationCredential]
(
	[Id] BIGINT IDENTITY(1,1),
	[AccountIdentifier] NVARCHAR(500) NULL,
	[Username] NVARCHAR(500) NULL,
	[IntegrationId] BIGINT NOT NULL,
	[Password] NVARCHAR(500) NULL,
	[Token] NVARCHAR(100) null,
	[CratedUTC] DATETIME NOT NULL,
	[LastModifiedUTC] DATETIME NULL, 
    CONSTRAINT [PK_IntegrationCredential] PRIMARY KEY (Id), 
    CONSTRAINT [FK_IntegrationCredential_Integration] FOREIGN KEY (IntegrationId) REFERENCES [Integration]([Id])
)
