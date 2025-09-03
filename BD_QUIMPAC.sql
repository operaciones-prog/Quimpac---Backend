use DBQUIMPAC_LOCAL

/*primero se ejecuta esta tabla*/
create table clientes(
cli_cod_cli int primary key identity (1,1),
cli_raz_soc varchar  (100)  not null,     /* Razon social*/
cli_cod_sap nvarchar (10)  not null,    /* C�d SAP (PK)*/
cli_pue_pla varchar  (100),               /* Puesto planificaci�n*/
cli_org_ven Nvarchar (100),               /* Organizaci�n de ventas*/
cli_cen     nvarchar (100),               /* Centro*/
cli_rut     nvarchar (100)  not null,     /* Ruta*/
cli_pue_exp varchar  (100)  not null,     /* Puesto expedici�n*/
cli_est     int             not null,     /* Estado*/
cli_fec_cre datetime,                     /* Fecha de creaci�n*/
cli_usu_cre_sap nvarchar (10)           ,     /* Usuario creador*/
cli_fec_mod datetime,                     /* Fecha de modificaci�n*/
cli_usu_mod_sap nvarchar (10),                   /* Usuario modificaci�n*/
);

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[clientes]') AND type in (N'U'))
drop table clientes

ALTER TABLE clientes
ADD CONSTRAINT fk_clientes_usu_cre foreign key (cli_usu_cre) references usuarios (usu_cod_usu);
ALTER TABLE clientes
add constraint fk_clientes_usu_mod foreign key (cli_usu_mod) references usuarios (usu_cod_usu);

/*segunda se ejecuta esta tabla*/
create table rol(
rol_cod     int primary key identity (1,1),  /* Cod. Rol*/
rol_nom     varchar  (100)   not null,       /* Nombre rol*/
rol_est     int ,                            /* Estado*/
rol_fec_cre datetime,                        /* Fecha de creaci�n*/
rol_usu_cre_sap nvarchar (10),                             /* Usuario creador*/
rol_fec_mod datetime,                        /* Fecha de modificaci�n*/
rol_usu_mod_sap nvarchar (10),                              /* Usuario modificaci�n*/
);

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rol]') AND type in (N'U'))
drop table rol

ALTER TABLE rol
ADD CONSTRAINT fk_rol_usu_cre foreign key (rol_usu_cre) references usuarios (usu_cod_usu);
ALTER TABLE rol
add constraint fk_rol_usu_mod foreign key (rol_usu_mod) references usuarios (usu_cod_usu);

/*tercero se ejecuta esta tabla*/
create table usuarios(
usu_cod_usu int primary key identity (1,1),  /* */
usu_cod_cli_sap nvarchar (10) ,
usu_nom_ape varchar   (100) not null,        /* Nombre y Apellidos*/
usu_usu     nvarchar  (10)  not null,        /* Usuario*/
usu_cla     nvarchar  (10)  not null,        /* Clave*/
usu_cor     nvarchar  (50)  not null,        /* Correo*/
usu_cod_sap char      (10)      not null,        /* C�d. SAP (FK)*/
usu_cod_rol int             ,                /* Cod. Rol*/
usu_est     int             ,                /* Estado*/
usu_fec_cre datetime,                        /* Fecha de creaci�n*/
usu_usu_cre_sap nvarchar (10),                             /* Usuario creador*/
usu_fec_mod datetime,                        /* Fecha de modificaci�n*/
usu_usu_mod_sap nvarchar (10),                             /* Usuario modificaci�n*/

);

ALTER TABLE usuarios
ADD CONSTRAINT fk_usu_cod_cli foreign key (usu_cod_cli) references clientes (cli_cod_cli);

alter table usuarios
  drop constraint fk_usu_cod_cli
ALTER TABLE usuarios
DROP COLUMN usu_cod_cli;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usuarios]') AND type in (N'U'))
drop table usuarios

/*cuarto se ejecuta esta tabla*/
create table permiso(   
per_cod     int primary key identity (1,1),  /* Cod. Permiso*/
per_nom     varchar   (100),                 /* Nombre de permiso*/
per_uri     nvarchar  (500),                 /* Uri de permiso*/
per_est     int,                             /* Estado*/
per_fec_cre datetime,                        /* Fecha de creaci�n*/
per_usu_cre_sap nvarchar (10),                             /* Usuario creador*/
per_fec_mod datetime,                        /* Fecha de modificaci�n*/
per_usu_mod_sap nvarchar (10),                              /* Usuario modificaci�n*/
);

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[permiso]') AND type in (N'U'))
drop table permiso

ALTER TABLE permiso
ADD CONSTRAINT fk_permiso_usu_cre foreign key (per_usu_cre) references usuarios (usu_cod_usu);
ALTER TABLE permiso
add constraint fk_permiso_usu_mod foreign key (per_usu_mod) references usuarios (usu_cod_usu);

/*quinto se ejecuta esta tabla*/
create table rol_permiso(
rol_per_cod int primary key identity (1,1),  /* */
rol_per_cod_rol int,                         /* Cod. Rol*/
rol_per_cod_per int,                         /* Cod. Permiso*/
rol_per_est     int,                         /* Estado*/
rol_per_fec_cre datetime,                    /* Fecha de creaci�n*/
rol_per_usu_cre_sap nvarchar (10),                         /* Usuario creador*/
rol_per_fec_mod datetime,                    /* Fecha de modificaci�n*/
rol_per_usu_mod_sap nvarchar (10),                         /* Usuario modificaci�n*/
);

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rol]') AND type in (N'U'))
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rol_permiso]') AND type in (N'U'))
drop table rol_permiso

ALTER TABLE rol_permiso
ADD CONSTRAINT fk_rol_permiso_cod_rol foreign key (rol_per_cod_rol) references rol (rol_cod);
ALTER TABLE rol_permiso
add constraint fk_rol_permiso_cod_per foreign key (rol_per_cod_per) references permiso (per_cod);
ALTER TABLE rol_permiso
add constraint fk_rol_permiso_usu_cre foreign key (rol_per_usu_cre) references usuarios (usu_cod_usu);
ALTER TABLE rol_permiso
add constraint fk_rol_permiso_usu_mod foreign key (rol_per_usu_mod) references usuarios (usu_cod_usu);

create table entrega(
ent_cod_ent int primary key identity (1,1),  /* key de entrega*/
ent_mat varchar(100),                        /* nombre del material*/
ent_uni_med varchar(50),                     /* unidad de medida de material*/
ent_alm int ,                                /* key de entrega*/
ent_lot int,                                 /* lote de entrega*/
ent_can int ,                                /* cantidad de entrega*/
ent_cho nvarchar (10),                       /* chofer de entrega*/
ent_nom_cho varchar (100),                   /* nombre de chofer*/
ent_rut int ,                                /* rut de entrega*/
ent_est int,                                 /* estado de entrega ENTREGADO-EN PROCESO*/
ent_pla nvarchar (20),                       /* placa de vehiculo de entrega*/
ent_fec_hor_ent datetime,                    /* fecha y hora de entrega*/
ent_fec_cre datetime,                        /* fecha de creacion de entrega*/
ent_usu_cre_sap nvarchar(10),                /* usuario creador de entrega*/
ent_fec_mod datetime,                        /* fecha modificacion de entrega*/
ent_usu_mod_sap nvarchar(10)                 /* usuario actualiza entrega*/
);

alter table entrega 
add ent_cod_pro as Concat(Replicate('0', 10 -len(ent_cod_ent)),ent_cod_ent) Persisted /* codigo de correlativo de producto de entrega*/