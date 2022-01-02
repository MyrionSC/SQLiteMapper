CREATE TABLE symboldata
(
    name TEXT,
    age  INTEGER,
    etag TEXT
);

INSERT INTO symboldata (name, age, etag)
VALUES ('Martin', 42, '@123@'),
       ('Casper', 2, '@456@');
