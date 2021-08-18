namespace NETDiscordBotTests.Preconditions
{
    using System.Threading.Tasks;
    using Discord.Commands;
    using FluentAssertions;
    using Moq;
    using NETDiscordBot.Preconditions;
    using Xunit;

    public class RequireAttachmentTests
    {
        [Fact]
        public async Task RequireAttachment_WithNoAttachments_ReturnsError()
        {
            var requireAttachmentPrecondition = new RequireAttachment();
            var context = new Mock<ICommandContext>();
            context
                .Setup(x => x.Message.Attachments.Count)
                .Returns(0);
            var result = await requireAttachmentPrecondition.CheckPermissionsAsync(
                                    context.Object,
                                    null,
                                    null);

            result.IsSuccess.Should().BeFalse();
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(123)]
        public async Task RequireAttachment_WithAttachments_ReturnsSuccess(int attachmentsCount)
        {
            var requireAttachmentPrecondition = new RequireAttachment();
            var context = new Mock<ICommandContext>();
            context
                .Setup(x => x.Message.Attachments.Count)
                .Returns(attachmentsCount);
            var result = await requireAttachmentPrecondition.CheckPermissionsAsync(
                context.Object,
                null,
                null);

            result.IsSuccess.Should().BeTrue();
        }
    }
}