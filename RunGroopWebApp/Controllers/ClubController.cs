﻿using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAllClubs();
            return View(clubs);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetClubById(id);
            return View(club);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel newClub)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(newClub.Image);

                var club = new Club
                {
                    Title = newClub.Title,
                    Description = newClub.Description,
                    Image = result.Url.ToString(),
                    ClubCategory = newClub.ClubCategory,
                    Address = new Address
                    {
                        Street = newClub.Address.Street,
                        City = newClub.Address.City,
                        State = newClub.Address.State,
                    }
                };
                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }

            return View(newClub);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetClubByIdNoTracking(id);

            if (club == null) return View("Error");

            var oldClub = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                ClubCategory = club.ClubCategory,
                AddressId = (int)club.AddressId,
                Address = club.Address,
            };
            return View(oldClub);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel newClub)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", newClub);
            }

            var club = await _clubRepository.GetClubByIdNoTracking(id);

            if (club == null)
            {
                return View("Error");
            }

            var photoResult = await _photoService.AddPhotoAsync(newClub.Image);

            if (photoResult.Error != null)
            {
                ModelState.AddModelError("Image", "Photo upload failed");
                return View(newClub);
            }

            if (!string.IsNullOrEmpty(club.Image))
            {
                _ = _photoService.DeletePhotoAsync(club.Image);
            }

            var updateClub = new Club
            {
                Id = id,
                Title = newClub.Title,
                Description = newClub.Description,
                Image = photoResult.Url.ToString(),
                AddressId = newClub.AddressId,
                Address = newClub.Address,
            };

            _clubRepository.Update(updateClub);

            return RedirectToAction("Index");
        }
    }
}