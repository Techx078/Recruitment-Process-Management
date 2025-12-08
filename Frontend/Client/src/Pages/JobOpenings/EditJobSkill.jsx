import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getAllSkills,
  getJobOpeningById,
  updateJobSkill,
} from "../../Services/JobOpeningService";

const EditJobSkill = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [skill, setSkill] = useState([]);
  const [selected, setSelected] = useState([]);
  
  useEffect(() => {
    const load = async () => {
      const token = localStorage.getItem("token");

      const allSkills = await getAllSkills(token);
      setSkill(allSkills);

      const job = await getJobOpeningById(id, token);
 
      const selected = job.jobSkills.map((j) => ({
        SkillName: j.name,
        minExperience: j.minExperience,
        IsRequired:j.isRequired
      }));
      setSelected(selected);
    };

    load();
  }, [id]);
const toggleSkill = (SkillName, isChecked) => {
  setSelected((prev) => {
    if (isChecked) {
      if (prev.some((s) => s.SkillName === SkillName)) {
        return prev;
      }
      return [
        ...prev,
        { SkillName:SkillName, IsRequired: false , minExperience : 0 }
      ];
    } 
    else {
      return prev.filter((s) => s.SkillName !== SkillName);
    }
  });
};
const toggleisRequired = (SkillName) => {
  setSelected((prev) =>
    prev.map((skill) =>
      skill.SkillName === SkillName
        ? { ...skill, IsRequired: !skill.IsRequired }
        : skill
    )
  );
};
const onChangeExperience = (SkillName) => (e) => {
  setSelected((prev) =>
    prev.map((skill) =>
      skill.SkillName === SkillName
        ? { ...skill, minExperience: Number(e.target.value) }
        : skill
    )
  );
};

  const save = async () => {
    const token = localStorage.getItem("token");
    console.log(selected);
    await updateJobSkill(id, selected, token);
    alert("skills updated!");
    navigate(`/job-openings/${id}`);
  };

  return (
    <div className="p-6 max-w-xl mx-auto bg-white shadow rounded">
      <h2 className="text-xl font-bold mb-4">Update Skills</h2>

      <div className="border p-3 rounded">
        <label className="block font-semibold mb-2">Required Skills</label>

          
     <div className="border p-3 rounded">
            <label className="block font-semibold mb-2">Required Skills</label>
            <div className="space-y-2 max-h-40 overflow-auto border p-2 rounded">
              {skill.map((s) => {
                const selectedSkill = selected.find(
                  (sk) => sk.SkillName === s.name
                );
                
                const isSelected = selectedSkill !== undefined;
                const isRequired = selectedSkill?.IsRequired || false;
                const minExperience = selectedSkill?.minExperience || 0;
                
                return (
                  <div
                    key={s.skillId}
                    className="flex items-center gap-4 p-2 hover:bg-gray-100 rounded"
                  >
                    {/* Skill checkbox + name */}
                    <label className="flex items-center gap-3 cursor-pointer min-w-[220px]">
                      <input
                        type="checkbox"
                        checked={isSelected}
                        onChange={(e) => toggleSkill(s.name,e.target.checked)}
                      />
                      <div>
                        <div className="font-medium">{s.name}</div>
                        <div className="text-xs text-gray-600">
                          Skill ID: {s.skillId}
                        </div>
                      </div>
                    </label>

                    {/* Required checkbox */}
                    {isSelected && (
                      <label className="flex items-center gap-1 text-sm cursor-pointer">
                        <input
                          type="checkbox"
                          checked={isRequired}
                          onChange={() => toggleisRequired(s.name)}
                        />
                        Required
                      </label>
                    )}

                    {/* Min experience input */}
                    {isSelected && isRequired && (
                      <div className="flex items-center gap-2">
                        <label className="text-sm whitespace-nowrap">
                          Min Exp:
                        </label>

                        <input
                          type="number"
                          name="minDomainExperience"
                          value={minExperience}
                          onChange={ onChangeExperience(s.name)}
                          className="w-24 border rounded px-2 py-1 text-sm"
                          placeholder="Years"
                          min={0}
                          step={1}
                        />
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          </div>
      </div>

      <button
        onClick={save}
        className="mt-4  bg-gray-900 text-white px-4 py-2 rounded"
      >
        Save Changes
      </button>

      <button
        onClick={() => navigate(-1)}
        className="mt-4 ml-3 bg-gray-500 text-white px-4 py-2 rounded"
      >
        Cancel
      </button>
    </div>
  );
};

export default EditJobSkill;
