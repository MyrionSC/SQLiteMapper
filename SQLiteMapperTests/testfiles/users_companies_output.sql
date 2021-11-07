CREATE TABLE companies
(
    companyId   INTEGER,
    companyName TEXT,
    Address     TEXT
);

CREATE TABLE users
(
    name     TEXT,
    age      INTEGER,
    worksFor INTEGER
);

INSERT INTO companies (companyId, companyName, Address)
VALUES (1, 'Commentor', 'Visionsvej 21'),
       (2, 'NetCompany', 'Havnen');

INSERT INTO users (name, age, worksFor)
VALUES ('Martin', 42, 1),
       ('Casper', 2, 1),
       ('Ali', 999, 2),
       ('Muhammed', null, 2);

