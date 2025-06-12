﻿using CodeAssistant.API.DTOs;
using CodeAssistant.API.Mappers;
using CodeAssistant.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks;
using System.Diagnostics;
using Analyzer.Core.Application.Interfaces;

namespace CodeAssistant.API.Controllers
{
    /// <summary>
    /// Controller for analyzing C# solution and returning diagnostic results.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SolutionAnalysisController : ControllerBase
    {
        private AnalyzeSolutionUseCase _analyzeSolutionUseCase;
        private readonly IZipHandler _zipHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref=SolutionAnalysisController"/> class.
        /// </summary>
        /// <param name="analyzeSolutionUseCase">The use case responsible for analyzing solutions.</param>
        /// <param name="zipHandler">The service responsible for saving and etracting zip file</param>
        public SolutionAnalysisController(AnalyzeSolutionUseCase analyzeSolutionUseCase, IZipHandler zipHandler)
        {
            _analyzeSolutionUseCase = analyzeSolutionUseCase;
            _zipHandler = zipHandler;
        }

        /// <summary>
        /// Analyzes the provided C# solution for compilation errors and warnings.
        /// Accepts .zip file containing C# solution
        /// </summary>
        /// <returns>
        /// A <see cref="AnalyzeSolutionResponseDto"/> containing the list of detected errors and warnings.
        /// Returns 400 Bad Request if input is null or if 
        /// </returns>
        /// <exception cref="Exception">If any exception ocurres, returns bad request containing the exception message</exception>
        [HttpPost("analyze")]
        //[Consumes("application/octet-stream")]
        public async Task<ActionResult> AnalyzeSolutionAsync()
        {
            // need to commit somwhting for cicd :)
            try
            {
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(memoryStream);
                    if (memoryStream.Length == 0)
                        return BadRequest("Empty request body");

                    fileBytes = memoryStream.ToArray();
                }

                var solutionPath = await _zipHandler.GetPathAsync(fileBytes);
                if (solutionPath == null)
                {
                    return BadRequest("No solution found in zip.");
                }

                var solution = AnalyzeSolutionMapper.ToModel(solutionPath);
                var result = await _analyzeSolutionUseCase.ExecuteAsync(solution);
                var response = AnalyzeSolutionMapper.ToDto(result);

                return Ok(response);
            }
            catch (Exception ex) 
            {
                return BadRequest($"Exception ocurred during analysis: {ex.Message}");
            }
        }


    }
}
