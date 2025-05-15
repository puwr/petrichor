using Application.Common.Interfaces;
using Domain.Images.Events;
using MediatR;

namespace Application.Images.Events;

public class ImageDeletedEventHandler(
    IUploadsRepository uploadsRepository,
    IThumbnailsRepository thumbnailsRepository) : INotificationHandler<ImageDeletedEvent>
{
    private readonly IUploadsRepository _uploadsRepository = uploadsRepository;
    private readonly IThumbnailsRepository _thumbnailsRepository = thumbnailsRepository;

    public async Task Handle(ImageDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _uploadsRepository.RemoveFileAsync(notification.ImagePath);
        await _thumbnailsRepository.RemoveThumbnailAsync(notification.ThumbnnailPath);
    }
}
