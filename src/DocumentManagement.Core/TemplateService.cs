using DocumentManagement.Models;
using DocumentManagement.Data.Repositories;

namespace DocumentManagement.Core.Services;

/// <summary>
/// 模板管理服务接口
/// </summary>
public interface ITemplateService
{
    Task<DocumentTemplate> CreateTemplateAsync(DocumentTemplate template);
    Task<DocumentTemplate?> GetTemplateAsync(int id);
    Task<DocumentTemplate?> GetTemplateWithControlsAsync(int id);
    Task<IEnumerable<DocumentTemplate>> GetAllTemplatesAsync();
    Task<IEnumerable<DocumentTemplate>> GetEnabledTemplatesAsync();
    Task<DocumentTemplate> UpdateTemplateAsync(DocumentTemplate template);
    Task DeleteTemplateAsync(int id);
    Task<TemplateControl> AddControlToTemplateAsync(int templateId, TemplateControl control);
    Task<IEnumerable<TemplateControl>> GetTemplateControlsAsync(int templateId);
    Task DeleteTemplateControlAsync(int controlId);
}

/// <summary>
/// 模板管理服务实现
/// </summary>
public class TemplateService : ITemplateService
{
    private readonly IDocumentTemplateRepository _templateRepository;
    private readonly ITemplateControlRepository _controlRepository;

    public TemplateService(
        IDocumentTemplateRepository templateRepository,
        ITemplateControlRepository controlRepository)
    {
        _templateRepository = templateRepository;
        _controlRepository = controlRepository;
    }

    public async Task<DocumentTemplate> CreateTemplateAsync(DocumentTemplate template)
    {
        template.CreatedAt = DateTime.Now;
        template.UpdatedAt = DateTime.Now;
        
        return await _templateRepository.AddAsync(template);
    }

    public async Task<DocumentTemplate?> GetTemplateAsync(int id)
    {
        return await _templateRepository.GetByIdAsync(id);
    }

    public async Task<DocumentTemplate?> GetTemplateWithControlsAsync(int id)
    {
        return await _templateRepository.GetWithControlsAsync(id);
    }

    public async Task<IEnumerable<DocumentTemplate>> GetAllTemplatesAsync()
    {
        return await _templateRepository.GetAllAsync();
    }

    public async Task<IEnumerable<DocumentTemplate>> GetEnabledTemplatesAsync()
    {
        return await _templateRepository.GetEnabledTemplatesAsync();
    }

    public async Task<DocumentTemplate> UpdateTemplateAsync(DocumentTemplate template)
    {
        var existing = await _templateRepository.GetByIdAsync(template.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"模板ID '{template.Id}' 不存在");
        }

        template.UpdatedAt = DateTime.Now;
        return await _templateRepository.UpdateAsync(template);
    }

    public async Task DeleteTemplateAsync(int id)
    {
        var existing = await _templateRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new InvalidOperationException($"模板ID '{id}' 不存在");
        }

        await _templateRepository.DeleteAsync(id);
    }

    public async Task<TemplateControl> AddControlToTemplateAsync(int templateId, TemplateControl control)
    {
        var template = await _templateRepository.GetByIdAsync(templateId);
        if (template == null)
        {
            throw new InvalidOperationException($"模板ID '{templateId}' 不存在");
        }

        control.TemplateId = templateId;
        return await _controlRepository.AddAsync(control);
    }

    public async Task<IEnumerable<TemplateControl>> GetTemplateControlsAsync(int templateId)
    {
        return await _controlRepository.GetByTemplateIdAsync(templateId);
    }

    public async Task DeleteTemplateControlAsync(int controlId)
    {
        var existing = await _controlRepository.GetByIdAsync(controlId);
        if (existing == null)
        {
            throw new InvalidOperationException($"控件ID '{controlId}' 不存在");
        }

        await _controlRepository.DeleteAsync(controlId);
    }
}