CREATE TABLE klanten.dbo.TblOfferline (
  Id int NOT NULL IDENTITY(1,1),
  OfferId int DEFAULT (NULL),
  VatRateId int DEFAULT (NULL),
  Currency VARCHAR(3),
  Amount float DEFAULT ((0)),
  SequenceNumber int DEFAULT ((0)),
  Description VARCHAR(8000)
)
CREATE INDEX TblOfferline$Amount ON klanten.dbo.TblOfferline (Amount)
CREATE INDEX TblOfferline$VatRateId ON klanten.dbo.TblOfferline (VatRateId)
CREATE INDEX TblOfferline$OfferId ON klanten.dbo.TblOfferline (OfferId)
CREATE UNIQUE INDEX TblOfferline$PrimaryKey ON klanten.dbo.TblOfferline (Id)