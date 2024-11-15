using Xunit;
using Moq;
using AutoMapper;
using FluentAssertions;
using AutoFixture;
using CineWorld.Services.MovieAPI.Controllers;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using CineWorld.Services.MovieAPI.Exceptions;
using System;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Test
{
  public class CategoryAPIControllerTest
  {
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUtil> _mockUtil;
    private readonly CategoryAPIController _controller;
    private readonly Fixture _fixture;

    public CategoryAPIControllerTest()
    {
      _mockUnitOfWork = new Mock<IUnitOfWork>();
      _mockMapper = new Mock<IMapper>();
      _mockUtil = new Mock<IUtil>();
      _controller = new CategoryAPIController(_mockUnitOfWork.Object, _mockMapper.Object, _mockUtil.Object);
      _fixture = new Fixture();

      _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
      _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task Get_ShouldReturnAllCategories()
    {
      var categories = _fixture.CreateMany<Category>(3);
      var categoryDtos = _fixture.CreateMany<CategoryDto>(3);

      _mockUnitOfWork.Setup(u => u.Category.GetAllAsync(null)).ReturnsAsync(categories);
      _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(It.IsAny<IEnumerable<Category>>())).Returns(categoryDtos);

      var result = await _controller.Get();

      var okResult = result.Result as OkObjectResult;
      okResult.Should().NotBeNull();
      var response = okResult.Value as ResponseDto;
      response.Should().NotBeNull();
      //response.TotalItems.Should().Be(3);
      response.Result.Should().BeEquivalentTo(categoryDtos);
    }

    [Fact]
    public async Task Get_WithValidId_ShouldReturnCategory()
    {
      var category = _fixture.Create<Category>();
      var categoryDto = _fixture.Create<CategoryDto>();

      _mockUnitOfWork.Setup(u => u.Category.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false))
          .ReturnsAsync(category);
      _mockMapper.Setup(m => m.Map<CategoryDto>(category)).Returns(categoryDto);

      var result = await _controller.Get(category.CategoryId);

      var okResult = result.Result as OkObjectResult;
      okResult.Should().NotBeNull();
      var response = okResult.Value as ResponseDto;
      response.Should().NotBeNull();
      response.Result.Should().BeEquivalentTo(categoryDto);
    }

    [Fact]
    public async Task Get_WithInvalidId_ShouldThrowNotFoundException()
    {
      _mockUnitOfWork.Setup(u => u.Category.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false)).ReturnsAsync((Category)null);

      Func<Task> act = async () => await _controller.Get(999);

      await act.Should().ThrowAsync<NotFoundException>().WithMessage("Category with ID: 999 not found.");
    }

    [Fact]
    public async Task Post_ShouldCreateCategory()
    {
      var categoryDto = _fixture.Create<CategoryDto>();
      var category = _fixture.Build<Category>().Without(c => c.Movies).Create();

      _mockMapper.Setup(m => m.Map<Category>(categoryDto)).Returns(category);
      _mockUnitOfWork.Setup(u => u.Category.AddAsync(category)).Returns(Task.CompletedTask);
      _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
      _mockMapper.Setup(m => m.Map<CategoryDto>(category)).Returns(categoryDto);

      var result = await _controller.Post(categoryDto);

      var createdResult = result.Result as CreatedResult;
      createdResult.Should().NotBeNull();
      var response = createdResult.Value as ResponseDto;
      response.Should().NotBeNull();
      response.Result.Should().BeEquivalentTo(categoryDto);
    }

    [Fact]
    public async Task Put_ShouldUpdateCategory()
    {
      var categoryDto = _fixture.Create<CategoryDto>();
      var category = _fixture.Create<Category>();

      _mockMapper.Setup(m => m.Map<Category>(categoryDto)).Returns(category);
      _mockUnitOfWork.Setup(u => u.Category.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false)).ReturnsAsync(category);
      _mockUnitOfWork.Setup(u => u.Category.UpdateAsync(category)).Returns(Task.CompletedTask);
      _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
      _mockMapper.Setup(m => m.Map<CategoryDto>(category)).Returns(categoryDto);

      var result = await _controller.Put(categoryDto);

      var okResult = result.Result as OkObjectResult;
      okResult.Should().NotBeNull();
      var response = okResult.Value as ResponseDto;
      response.Should().NotBeNull();
      response.Result.Should().BeEquivalentTo(categoryDto);
    }

    [Fact]
    public async Task Put_WithInvalidId_ShouldThrowNotFoundException()
    {
      _mockUnitOfWork.Setup(u => u.Category.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false))
          .ReturnsAsync((Category)null);

      var categoryDto = _fixture.Create<CategoryDto>();

      Func<Task> act = async () => await _controller.Put(categoryDto);

      await act.Should().ThrowAsync<NotFoundException>()
          .WithMessage($"Category with ID: {categoryDto.CategoryId} not found.");
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
      // Arrange
      var category = _fixture.Create<Category>();  // Tạo một category giả định

      _mockUnitOfWork.Setup(u => u.Category.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false))
          .ReturnsAsync(category);
      _mockUnitOfWork.Setup(u => u.Category.RemoveAsync(category)).Returns(Task.CompletedTask);
      _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.Delete(category.CategoryId);

      // Assert
      result.Should().BeOfType<NoContentResult>();  // Kiểm tra nếu trả về NoContentResult
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldThrowNotFoundException()
    {
      // Arrange
      _mockUnitOfWork.Setup(u => u.Category.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false))
          .ReturnsAsync((Category)null);  // Giả lập danh mục không tồn tại

      // Act
      Func<Task> act = async () => await _controller.Delete(999);  // Sử dụng ID không hợp lệ

      // Assert
      await act.Should().ThrowAsync<NotFoundException>()
          .WithMessage("Category with ID: 999 not found.");  // Kiểm tra ngoại lệ NotFoundException với thông báo chính xác
    }


  }
}
