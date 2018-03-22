CREATE TABLE [dbo].[IntegrationLog]
(
	[Id] BIGINT IDENTITY(1,1), 
	[IntegrationId] BIGINT NOT NULL,
	[IntegrationErrorNumber] BIGINT NULL,
    CONSTRAINT [FK_IntegrationLog_Integration] FOREIGN KEY (IntegrationId) REFERENCES [Integration](Id), 
    CONSTRAINT [FK_IntegrationLog_IntegrationErrorCode] FOREIGN KEY (IntegrationErrorNumber) REFERENCES [IntegrationErrorCode](Number)
)
