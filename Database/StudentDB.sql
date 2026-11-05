create database StudentDB;
go

use StudentDB;
go

CREATE TABLE Students
(
Id INT IDENTITY(1,1) PRIMARY KEY,
NRP VARCHAR(10) NOT NULL,
Nama VARCHAR(100) NOT NULL,
Jurusan VARCHAR(100) NOT NULL,
Gender VARCHAR(20) NOT NULL,
Email VARCHAR(100)
);

INSERT INTO Students
(NRP, Nama, Jurusan, Gender, Email)
VALUES
('5022201029','Kamarudin','Teknik Elektro','Laki-laki','rudin@gmail.com'),
('5023231029','Siti Aminah','Teknik Biomedik','Perempuan','siti@gmail.com'),
('5024211029','Andi Wijaya','Teknik Komputer','Laki-laki','andi@gmail.com'),
('5025251029','Rina Sari','Teknik Informatika','Perempuan','rina@gmail.com'),
('5026251029','Bambang Pamungkas','Sistem Informasi','Perempuan','mbang@gmail.com'),
('5027251029','Budi Sudarsono','Teknologi Informasi','Perempuan','budi@gmail.com');