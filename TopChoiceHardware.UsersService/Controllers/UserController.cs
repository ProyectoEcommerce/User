﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TopChoiceHardware.UsersService.Application.Services;
using TopChoiceHardware.UsersService.Domain.DTOs;
using TopChoiceHardware.UsersService.Domain.Entities;

namespace TopChoiceHardware.UsersService.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly IMapper _mapper;

        public UserController(IUsuarioService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuccesfulRegisterDto), StatusCodes.Status201Created)]
        public IActionResult Post(UsuarioDtoForCreation usuario)
        {
            try
            {
                var usuarioEntity = _service.CreateUsuario(usuario);

                if (usuarioEntity != null)
                {
                    return new JsonResult(new SuccesfulRegisterDto(usuarioEntity)) { StatusCode = 201 };
                }

                return new JsonResult(new UnsuccesfulRegisterDto()) { StatusCode = 409 };
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UsuarioDtoForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllUsers()
        {
            try
            {
                var usuarios = _service.GetUsuarios();
                var usuariosMapeados = _mapper.Map<List<UsuarioDtoForDisplay>>(usuarios);

                return new JsonResult(usuariosMapeados) {StatusCode = 200 };
            }
            catch (Exception)
            {

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuarioDtoForDisplay), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var usuario = _service.GetUsuarioById(id);
                var usuarioMapeado = _mapper.Map<UsuarioDtoForDisplay>(usuario);

                if (usuario == null)
                {
                    return NotFound();
                }

                return new JsonResult(usuarioMapeado) {StatusCode = 200 };
            }
            catch (Exception)
            {

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateUsuario(int id, [FromBody] UsuarioDtoForCreation usuario)
        {
            try
            {
                if (usuario == null)
                {
                    return BadRequest("Todos los campos deben estar completos para poder realizar la actualización de este elemento.");
                }

                var usuarioEntity = _service.GetUsuarioById(id);

                if (usuarioEntity == null)
                {
                    return NotFound();
                }

                _mapper.Map(usuario, usuarioEntity);
                _service.UpdateUsuario(usuarioEntity);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UserLogin(LoginDto loginUser)
        {
            try
            {
                var usuario = _service.GetUsuarioByEmailAndPassword(loginUser.Email, loginUser.Password);

                if(usuario != null)
                {
                    return new JsonResult(new SuccesfulLoginDto()) {StatusCode = 201 };
                }

                return new JsonResult(new UnsuccesfulLoginDto()) { StatusCode = 404 };
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
