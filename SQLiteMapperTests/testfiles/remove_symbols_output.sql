CREATE TABLE symboldata
(
    name TEXT,
    age  INTEGER,
    odata_etag TEXT
);

INSERT INTO symboldata (name, age, odata_etag)
VALUES ('Martin', 42, '@123@'),
       ('Casper', 2, '@456@');
