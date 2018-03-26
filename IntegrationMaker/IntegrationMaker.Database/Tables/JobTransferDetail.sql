CREATE TABLE [dbo].[JobTransferDetail]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,
	[JobId] BIGINT NOT NULL,
	[TableName] NVARCHAR(MAX) NOT NULL,
	[RowsTransferred] INT NOT NULL, 
    CONSTRAINT [FK_JobTransferDetail_Job] FOREIGN KEY (JobId) REFERENCES [Job]([Id])
)
