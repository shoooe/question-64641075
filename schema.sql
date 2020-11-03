CREATE TABLE users (
    id serial primary key,
    full_name text not null
);

CREATE TABLE posts (
    id serial primary key,
    title text not null,
    body text not null,
    author_id int not null
        references users (id)
            on update cascade
            on delete cascade,
    score int not null default 0
);

INSERT INTO users (id, full_name)
VALUES 
    (1, 'John Doe'),
    (2, 'Mary Doe'),
    (3, 'John Wick');

INSERT INTO posts (id, author_id, title, body, score)
VALUES 
    (1, 1, 'T1', 'B1', 34 ),
    (2, 1, 'T2', 'B2', 23 ),
    (3, 1, 'T3', 'B3', 12 ),
    (4, 1, 'T4', 'B4', 98 ),
    (5, 1, 'T5', 'B5', 76 ),
    (6, 1, 'T6', 'B6', 34 ),
    (7, 1, 'T7', 'B7', 71 ),
    (8, 1, 'T8', 'B8', 73 ),
    (9, 2, 'T9', 'B9', 91 ),
    (10, 2, 'T10', 'B10', 3 ),
    (11, 3, 'T11', 'B11', 8 ),
    (12, 3, 'T12', 'B12', 67 ),
    (13, 3, 'T14', 'B13', 340 ),
    (14, 3, 'T14', 'B14', 1 );