// See https://aka.ms/new-console-template for more information

using HotelListing.Data;


HotelContext ctx = new HotelContext();

// https://docs.microsoft.com/en-us/ef/core/managing-schemas/ensure-created
//ctx.Database.EnsureDeleted();
ctx.Database.EnsureCreated();
//Console.ReadKey();