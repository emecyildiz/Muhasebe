UPDATE MasrafTalebi SET Durum = '1' WHERE Durum = 'Bekliyor';
UPDATE MasrafTalebi SET Durum = '2' WHERE Durum = 'Onaylandi';
UPDATE MasrafTalebi SET Durum = '3' WHERE Durum = 'Reddedildi';
ALTER TABLE MasrafTalebi DROP CONSTRAINT CHK_TalepDurum;
ALTER TABLE MasrafTalebi DROP CONSTRAINT DF__MasrafTal__Durum__4F7CD00D;
ALTER TABLE MasrafTalebi ALTER COLUMN Durum INT NOT NULL;
ALTER TABLE MasrafTalebi ADD CONSTRAINT DF_MasrafTalebi_Durum DEFAULT 1 FOR Durum;
