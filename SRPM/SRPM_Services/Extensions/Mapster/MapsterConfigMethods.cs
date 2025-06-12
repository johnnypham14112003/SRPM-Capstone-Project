using SRPM_Repositories.Models;

namespace SRPM_Services.Extensions.Mapster;
public class MapsterConfigMethods
{
    // Hàm xử lý logic để lấy TypeObjectId dựa trên Type
    public static Guid? GetTypeObjectIdByType(Notification notification)
    {
        return notification.Type.ToLower() switch
        {
            "transaction" => notification.TransactionId,
            "individualevaluation" => notification.IndividualEvaluationId,
            "evaluationstage" => notification.EvaluationStageId,
            "evaluation" => notification.EvaluationId,
            "userrole" => notification.UserRoleId,
            "document" => notification.DocumentId,
            "membertask" => notification.MemberTaskId,
            "task" => notification.TaskId,
            //"systemconfiguration"
            _ => notification.SystemConfigurationId
        };
    }
}
