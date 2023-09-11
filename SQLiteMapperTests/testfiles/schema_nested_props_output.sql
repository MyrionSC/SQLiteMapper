CREATE TABLE IF NOT EXISTS [mytable] (
    [userId] TEXT,
    [groups] TEXT,
    [userData.id] TEXT,
    [userData.givenName] TEXT NULL,
    [userData.surname] TEXT NULL,
    [userData.fullName] TEXT,
    [userData.mail] TEXT NULL,
    [userData.preferredLanguage] TEXT NULL,
    [userData.appRole] TEXT,
    [userData.externalUserState] TEXT NULL,
    [userData.apiUser] TEXT,
    [userData.newsletterSubscriber] INT
);

INSERT INTO [mytable] VALUES
    ('xxxx','["yyyy"]','xxxx',NULL,NULL,' ','mail@test.dk','da','App.Viewer','Accepted','',FALSE);
