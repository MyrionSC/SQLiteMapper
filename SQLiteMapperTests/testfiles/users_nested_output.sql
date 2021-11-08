CREATE TABLE users
(
    name TEXT,
    age  INTEGER,
    isAlive INTEGER,
    address_city TEXT,
    address_street TEXT
);

INSERT INTO users (name, age, isAlive, address_city, address_street)
VALUES ('Martin', 42, 1, 'Aalborg', 'Søndergade 58 2th'),
       ('Casper', 2, 1, 'Aarhus', 'Gadenavn 42');
