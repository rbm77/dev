"use client";

import { useId, useState } from "react";
import { useController, type Control, type FieldPath, type FieldValues } from "react-hook-form";
import { Field, FieldDescription, FieldError, FieldLabel } from "@/components/ui/field";
import { EntityCombobox } from "@/components/shared/entity-combobox";

type ComboboxFieldProps<T extends FieldValues> = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any -- matches react-hook-form's own Control default
  control: Control<T, any, any>;
  name: FieldPath<T>;
  label: string;
  endpoint: string;
  /** Label of the value the form starts with (edit forms). */
  initialLabel?: string;
  placeholder?: string;
  searchPlaceholder?: string;
  description?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
};

/** Server-searched select (students, users) bound to react-hook-form. The value is the id as a string. */
export function ComboboxField<T extends FieldValues>({
  control,
  name,
  label,
  endpoint,
  initialLabel,
  placeholder,
  searchPlaceholder,
  description,
  required,
  disabled,
  className,
}: ComboboxFieldProps<T>) {
  const id = useId();
  const { field, fieldState } = useController({ control, name });
  const [selectedLabel, setSelectedLabel] = useState(initialLabel);

  return (
    <Field data-invalid={fieldState.invalid} className={className}>
      <FieldLabel htmlFor={id}>
        {label}
        {required && (
          <span className="text-destructive" aria-hidden>
            *
          </span>
        )}
      </FieldLabel>
      <EntityCombobox
        id={id}
        endpoint={endpoint}
        value={field.value ? String(field.value) : ""}
        selectedLabel={selectedLabel}
        placeholder={placeholder}
        searchPlaceholder={searchPlaceholder}
        disabled={disabled}
        invalid={fieldState.invalid}
        onSelect={(item) => {
          setSelectedLabel(item?.label);
          field.onChange(item?.value ?? "");
          field.onBlur();
        }}
      />
      {description && <FieldDescription>{description}</FieldDescription>}
      <FieldError errors={[fieldState.error]} />
    </Field>
  );
}
