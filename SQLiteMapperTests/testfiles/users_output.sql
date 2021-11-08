CREATE TABLE users
(
    name TEXT,
    age  INTEGER,
    isAlive INTEGER
);

INSERT INTO users (name, age, isAlive)
VALUES ('Martin', 42, 1),
       ('Casper', 2, 1),
       ('Ali', 999, 1),
       ('Muhammed', null, 0);
