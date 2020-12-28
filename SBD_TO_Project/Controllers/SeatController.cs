﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SBD_TO_Project.Data;
using SBD_TO_Project.Models;
using SBD_TO_Project.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SBD_TO_Project.Controllers
{
    public class SeatController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SeatController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int Id)
        {
            List<Seat> objList = _db.Seat.Where(sr => sr.ScreeningRoom.Id == Id).ToList();
            return View(objList);
        }

        public IActionResult Create(int Id)
        {
            ScreeningRoom screeningRoom = _db.ScreeningRoom.Find(Id);
            List<List<Seat>> seats = new List<List<Seat>>();
            for (int i = 0; i < screeningRoom.NumberOfRows; i++)
            {
                List<Seat> tempList = new List<Seat>();
                for (int j = 0; j < screeningRoom.NumberOfSeatsPerRow; j++)
                {
                    tempList.Add(new Seat()
                    {
                        RowNumber = i,
                        SeatNumber = j,
                        IdScreeningRoom = Id,
                        IsValid = true
                    });
                }
                seats.Add(tempList);
            }

            SeatVM obj = new SeatVM() { IdCinema = (int)screeningRoom.IdCinema, Seats = seats };
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SeatVM seatVM)
        {
            foreach(var row in seatVM.Seats)
            {
                foreach(var seat in row)
                {
                    _db.Add(seat);
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "ScreeningRoom", new { id = seatVM.IdCinema });
        }

        public IActionResult Edit(int Id)
        {
            var obj = _db.ScreeningRoom.Find(Id);
            if (obj == null)
                return NotFound();

            List<Seat> seats = _db.Seat.Where(s => s.IdScreeningRoom == obj.Id).ToList();
            List<List<Seat>> listOfSeats = new List<List<Seat>>();

            for (int i = 0; i < obj.NumberOfRows; i++)
            {
                List<Seat> tempList = new List<Seat>();
                for (int j = 0; j < obj.NumberOfSeatsPerRow; j++)
                {
                    tempList.Add(seats[j + i * obj.NumberOfSeatsPerRow]);
                }
                listOfSeats.Add(tempList);
            }

            SeatVM seatVM = new SeatVM() { IdCinema = (int)obj.IdCinema, Seats = listOfSeats };

            return View(seatVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SeatVM seatVM)
        {
            foreach (var row in seatVM.Seats)
            {
                foreach (var seat in row)
                {
                    _db.Update(seat);
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "ScreeningRoom", new { id = seatVM.IdCinema });
        }

    }
}
